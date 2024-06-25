import mercantile


def geo2xy(lat, lon, zoom):
    tile = mercantile.tile(lat, lon, zoom)
    return tile.x, tile.y


import requests, os
from flask import Flask, request, jsonify

app = Flask(__name__.split(".")[0])

url_nws = "https://api.weather.gov"
url_ows_map = "https://tile.openweathermap.org/map"


@app.route("/point", methods=["POST"])
def handle_point():
    data = request.get_json()
    return requests.get(f"{url_nws}/points/{data['lat']},{data['lon']}").json()


@app.route("/map", methods=["POST"])
def handle_map():
    data = request.get_json()
    x, y = geo2xy(float(data["lat"]), float(data["lon"]), 8)
    return requests.get(
        f"{url_ows_map}/radar/8/{x}/{y}/.png?appid={os.environ.get('OPEN_WEATHER_KEY')}"
    ).json()
    return jsonify(data)
