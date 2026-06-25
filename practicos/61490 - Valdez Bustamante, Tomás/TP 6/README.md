TP6 Asistente de IA

Configurar la clave de API

1-Copiá la plantilla a un archivo .env:

   cp .env.ejemplo .env       

2- Abrí .env y completá los datos del proveedor que vayas a usar. Por ejemplo, para Groq:

   GROQ_API_URL=https://api.groq.com/openai/v1/chat/completions
   GROQ_API_KEY=tu_clave_real_aca
   GROQ_MODEL=llama-3.3-70b-versatile
 

Ejecutar

Usa OPENAI por defecto:
dotnet run asistente.cs

Elegir otro proveedor (usa GROQ_*, GEMINI_*, etc. según el nombre):
dotnet run asistente.cs -- groq

Usar el asistente:

- Escribí en el campo de abajo y Enter (o el botón Enviar) para mandar el mensaje.
- La respuesta aparece a medida que se genera.
- Mientras responde, la entrada se deshabilita para no superponer envíos.
- Esc cierra la aplicación.
- Probá las herramientas de archivos:
  "lista los archivos de esta carpeta"
  "Leé el archivo AGENTS.md"
  "Guardá un hola mundo en C# en `hola.cs`"



