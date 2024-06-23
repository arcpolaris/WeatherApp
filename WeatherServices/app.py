from flask import Flask, request, jsonify

app = Flask(__name__.split(".")[0])


@app.route("/")
def hello_world():
    return "Hello, World!"


@app.route("/api", methods=["POST"])
def handle_request():
    data = request.get_json()
    return jsonify(data)


if __name__ == "__main__":
    app.run(debug=True)
