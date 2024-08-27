import requests
import json

url = "https://weatherapp-8jw4.onrender.com"


def prettyjson(d):
    return json.dumps(d, indent=4, sort_keys=True)


def log(r, d):
    response = requests.post(f"{url}/{r}", json=d)
    print(f"Status Code: {response.status_code}")
    #print(response.text)
    return response


# data = {"address": "Adams, Adams County, Wisconsin"}

# response = log("geocode", data)

# response = requests.post(f"{url}/grid", json=data)

# print(f"Status Code: {response.status_code}")
# print(prettyjson(response.json()))

# response = requests.post(f"{url}/zone", json=data)

# print(f"Status Code: {response.status_code}")
# print(prettyjson(response.json()))

# data = response.json()
# response = requests.post(f"{url}/alerts?now", json=data)

# print(f"Status Code: {response.status_code}")
# print(prettyjson(response.json()))

print(" === Notification Wizard === \n\n")
data = {"title" : input("Enter notification title...\n\n\t"),
        "subtitle" : input("Enter notification subtitle...\n\n\t"),
        "description" : input("Enter notification description...\n\n\t")}

input(f"Payload : {data}\nPress ENTER to send...\n")
print("\n")
log("publish", data)
# while True:
#     input("Press enter to trigger an event")
#     data = {"title": "Test Title", "subtitle": "Subtitle", "description": "Description"}
#     log("publish", data)