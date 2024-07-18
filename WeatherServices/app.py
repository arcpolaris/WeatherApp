import requests, secrets
from requests.structures import CaseInsensitiveDict as CIDict
from datetime import datetime, timezone
from os import environ as env
from flask import Flask, request, jsonify
from flask_socketio import SocketIO, emit

app = Flask(__name__.split(".")[0])
app.config["SECRET_KEY"] = secrets.token_hex(16)
socketio = SocketIO(app)

# Publishes an event to every subscribed client
# The filtering of events will happen on
# the client side because I hate python
@app.route("/publish", methods=["POST"])
def publish():
    data = request.json
    socketio.emit("alert event", data)
    return "", 200

# Gets a property of a point
def point_property(lat, long, key):
    url = f"https://api.weather.gov/points/{lat},{long}"
    response = requests.get(url).json()
    return response["properties"][key]


# Convert address to lat/long
def geocode(text):
    url = f"https://api.geoapify.com/v1/geocode/search?text={text}&apiKey={env['GEOAPIFY_KEY']}"
    headers = CIDict()
    headers["Accept"] = "application/json"
    response = requests.get(url, headers=headers).json()
    properties = response["features"][0]["properties"]
    return properties["lat"], properties["lon"]


@app.route("/geocode", methods=["POST"])
def handle_geocode():
    data = request.get_json()
    addr = data["address"]
    lat, long = geocode(addr)
    return jsonify({"lat": lat, "long": long})


# Convert lat/long to a grid point
def geo2grid(lat, long):
    url = f"https://api.weather.gov/points/{lat},{long}"
    response = requests.get(url).json()
    return (
        response["properties"]["gridX"],
        response["properties"]["gridY"],
        response["properties"]["gridId"],
    )


# Routing for geo2grid
@app.route("/grid/geo", methods=["POST"])
def handle_geo2grid():
    data = request.get_json()
    lat, long = data["lat"], data["long"]
    x, y, gid = geo2grid(lat, long)
    return jsonify({"gridX": x, "gridY": y, "gridId": gid})


# Convert address to a grid point
@app.route("/grid/addr", methods=["POST"])
def handle_addr2grid():
    data = request.get_json()
    addr = data["address"]
    lat, long = geocode(addr)
    x, y, gid = geo2grid(lat, long)
    return jsonify({"gridX": x, "gridY": y, "gridId": gid})


# Converts address or lat/long to a grid point
@app.route("/grid", methods=["POST"])
def handle_grid():
    data = request.get_json()
    if "lat" in data and "long" in data:
        lat, long = data["lat"], data["long"]
    elif "address" in data:
        addr = data["address"]
        lat, long = geocode(addr)
    else:
        return (
            jsonify({"error": "No valid address or geographic coordinates provided"}),
            400,
        )

    x, y, gid = geo2grid(lat, long)
    return jsonify({"gridX": x, "gridY": y, "gridId": gid})


# Convert lat/long to a zone
def geo2county(lat, long):
    return point_property(lat, long, "county")


# Routing for geo2grid
@app.route("/zone/geo", methods=["POST"])
def handle_geo2county():
    data = request.get_json()
    lat, long = data["lat"], data["long"]
    return jsonify({"zone": geo2county(lat, long)})


# Convert address to a county
@app.route("/zone/addr", methods=["POST"])
def handle_addr2county():
    data = request.get_json()
    addr = data["address"]
    lat, long = geocode(addr)
    return jsonify({"zone": geo2county(lat, long)})


# Converts address or lat/long to a county
@app.route("/zone", methods=["POST"])
def handle_county():
    data = request.get_json()
    if "lat" in data and "long" in data:
        lat, long = data["lat"], data["long"]
    elif "address" in data:
        addr = data["address"]
        lat, long = geocode(addr)
    else:
        return (
            jsonify({"error": "No valid address or geographic coordinates provided"}),
            400,
        )

    return jsonify({"zone": geo2county(lat, long)})


def filter_alerts(alerts):
    now = datetime.now(timezone.utc)
    filtered_alerts = []
    for alert in alerts:
        ends = alert["properties"].get("ends")
        if ends:
            if datetime.fromisoformat(ends).replace(tzinfo=timezone.utc) > now:
                filtered_alerts.append(alert)
        else:
            filtered_alerts.append(alert)
    return filtered_alerts


@app.route("/alerts", methods=["POST"])
def handle_alerts():
    data = request.get_json()
    url = "https://api.weather.gov/alerts/active?area=WI"
    response = requests.get(url).json()["features"]
    if "now" in request.args:
        response = filter_alerts(response)
    if "address" in request.args:
        lat, long = geocode(request.args["address"])
        zone = point_property(lat, long, "county")
        response = [i for i in response if zone in i["properties"]["affectedZones"]]
    if "lat" in request.args and "long" in request.args:
        lat, long = request.args["lat"], request.args["long"]
        zone = point_property(lat, long, "county")
        response = [i for i in response if zone in i["properties"]["affectedZones"]]
    if "zone" in data:
        response = [
            i for i in response if data["zone"] in i["properties"]["affectedZones"]
        ]
    return jsonify(response)
