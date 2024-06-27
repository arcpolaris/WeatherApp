import requests
from requests.structures import CaseInsensitiveDict as CIDict
from os import environ as env
from flask import Flask, request, jsonify

app = Flask(__name__.split(".")[0])


# Get all alerts in Wisconsin
@app.route("/alerts", methods=["POST"])
def handle_alerts():
    data = request.get_json()
    url = "https://api.weather.gov/alerts/active?area=WI"
    response = requests.get(url).json()
    return response


# Convert address to lat/long
def geocode(text):
    url = f"https://api.geoapify.com/v1/geocode/search?text={text}&apiKey={env['GEOAPIFY_KEY']}"
    headers = CIDict()
    headers["Accept"] = "application/json"
    response = requests.get(url, headers=headers).json()
    features = response["features"][0]
    return features["lat"], features["lon"]


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
