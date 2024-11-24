# Videohra v Unity - Tower Defense

Ročníková práce 3.E š. r. 2024/2025

<div>
    <img src="https://github.com/victoriousoft/tower-defense/actions/workflows/build.yml/badge.svg" alt="build status badge"/>
</div>

## Členové týmu
 - Kristián Kunc (Web, Deployment, Unity)
 - Viktor Jakovec (Unity, Web)
 - Leon Kubota (Textury, Animace)

## Co kde najdu?
Repozitář obsahuje videohru v Unity enginu a používá jím stanovenou strukturu souborů s následujícími doplňky:
 - Dokumentace: [`docs/`](docs/)
 - Webová stránka: [`🔗 td-web/`](https://github.com/victoriousoft/td-web/tree/main/)
 - Modely/rendery: [`🔗 blender/`](https://github.com/victoriousoft/blender)


## Jak spustit lokálně?
Nejjednoduší je použít Docker a docker-compose. Docker image obsahuje minimální NGINX web server, který slouží k zobrazení webové stránky.

```bash
docker-compose up
```

Webová stránka bude dostupná na adrese `http://localhost:8080/`.


**Detailnější informace o všech aspektech projektu najdete v dokumentaci.**
