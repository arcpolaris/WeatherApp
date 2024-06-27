import requests
from flask import Flask, request, jsonify

app = Flask(__name__.split(".")[0])

url = "https://api.weather.gov"


@app.route("/alerts", methods=["POST"])
def handle_alerts():
    data = request.get_json()
    return requests.get(f"{url}/alerts/active?area=WI").json()
