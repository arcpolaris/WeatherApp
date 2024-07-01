import requests
from requests.structures import CaseInsensitiveDict as CIDict
from datetime import datetime, timezone
from os import environ as env
from flask import Flask, request, jsonify

app = Flask(__name__.split(".")[0])


# Filter out inactive alerts
def filter_alerts(alerts):
    now = datetime.now(timezone.utc)
    filtered_alerts = [
        alert
        for alert in alerts
        if datetime.fromisoformat(alert["properties"]["ends"][:-1]).replace(
            tzinfo=timezone.utc
        )
        > now
    ]
    return filtered_alerts


# Get all alerts in Wisconsin
@app.route("/alerts", methods=["GET", "POST"])
def handle_alerts():
    data = request.get_json()
    url = "https://api.weather.gov/alerts/active?area=WI"
    response = requests.get(url).json()["features"]
    if "now" in request.args:
        response = filter_alerts(response)
    return response


# Convert address to lat/long
def geocode(text):
    url = f"https://api.geoapify.com/v1/geocode/search?text={text}&apiKey={env['GEOAPIFY_KEY']}"
    headers = CIDict()
    headers["Accept"] = "application/json"
    response = requests.get(url, headers=headers).json()
    properties = response["features"][0]["properties"]
    return properties["lat"], properties["lon"]


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
