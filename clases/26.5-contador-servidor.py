from fastapi import FastAPI
import uvicorn
import os

app = FastAPI()

contador = 0

@app.get("/contador")
def leer_contador():
    return {"contador": contador}

@app.post("/contador")
def incrementar_contador():
    global contador
    contador += 1

@app.delete("/contador")
def borrar_contador():
    global contador
    contador = 0

os.system("cls" if os.name == "nt" else "clear")
print("=== Servidor de Contador (Python) ===\n")
uvicorn.run(app, host="localhost", port=5001)