from flask import Flask
import psutil

app = Flask(__name__)

@app.route('/temperature')
def get_temperature():
    temperature = psutil.sensors_temperatures().get('cpu_thermal', None)
    if temperature:
        return str(temperature[0].current)
    else:
        return "Temperature data not available"

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
