# Schritt 1: Verwenden des offiziellen Go-Images als Basis
FROM golang:1.24.5-alpine AS builder

# Schritt 2: Setze das Arbeitsverzeichnis im Container
WORKDIR /app

# Schritt 3: Kopiere alle Go-Dateien in das Container-Verzeichnis
COPY . .

# Schritt 4: Hole alle Abhängigkeiten, die in go.mod definiert sind
RUN go mod tidy

# Schritt 5: Baue die Go-Anwendung
RUN go build -o main .

# Schritt 6: Erstelle ein schlankes Final-Image, indem du nur die notwendige Binärdatei übernimmst
FROM alpine:latest

# Schritt 7: Installiere minimale Abhängigkeiten für die Go-Anwendung (optional)
RUN apk --no-cache add ca-certificates

# Schritt 8: Kopiere die kompilierten Binärdateien aus dem Build-Image
COPY --from=builder /app/main /main

# Schritt 9: Setze den Befehl zum Starten der Anwendung
CMD ["/main"]

# Schritt 10: Exponiere einen Port (optional, falls du einen Server betreibst)
EXPOSE 80