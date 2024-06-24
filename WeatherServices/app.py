import requests
from flask import Flask, request, jsonify

app = Flask(__name__.split(".")[0])

url = "https://api.weather.gov/"

# @app.route("/")
# def hello_world():
#    return "Hello, World!"


@app.route("/api", methods=["POST"])
def handle_request():
    data = request.get_json()
    query = data["query"]
    if query == "point":
        return requests.get(f"{url}/points/{data['latitude']},{data['longitude']}")
    return jsonify(data)


# if __name__ == "__main__":
#    app.run(debug=True)
