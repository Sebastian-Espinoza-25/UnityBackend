from flask import Flask, jsonify, request
import random

app = Flask(__name__)

# Predefinir la ruta
path = [
    {"latitude": 0, "longitude": 0},
    {"latitude": 0, "longitude": -25},
    {"latitude": 25, "longitude": -25},
    
]

@app.route('/get_route', methods=['POST'])
def get_route():
    # Obtener los datos de inicio y destino del request
    data = request.get_json()
    start = data.get('start')
    destination = data.get('destination')

    # En este caso, estamos devolviendo la ruta predefinida,
    # pero puedes calcular dinámicamente la ruta entre el inicio y el destino
    # basándote en esos puntos.
    
    # Aquí simplemente devolvemos la ruta completa como ejemplo
    return jsonify(path)

if __name__ == '__main__':
    app.run(debug=True)
