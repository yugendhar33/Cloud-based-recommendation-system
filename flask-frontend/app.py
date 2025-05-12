from flask import Flask, request, render_template
import json
import os
import requests

app = Flask(__name__)

API_URL = "https://b7kr99wzvb.execute-api.us-east-1.amazonaws.com/prod/recommend"

@app.route("/", methods=["GET"])
def index():
    return render_template("index.html")

@app.route("/recommend", methods=["POST"])
def recommend():
    skills = [s.strip() for s in request.form.get("skills", "").split(",")]
    goal = request.form.get("goal", "").strip()

    payload = {
        "skills": skills,
        "goal": goal
    }

    try:
        response = requests.post(API_URL, json=payload)
        response.raise_for_status()
        recommendations = response.json()
    except requests.exceptions.RequestException as e:
        print(f"Error calling API: {e}")
        recommendations = []

    return render_template("index.html", recommendations=recommendations)

if __name__ == "__main__":
    port = int(os.environ.get("PORT", 8000))
    app.run(host="0.0.0.0", port=port, debug=True)
