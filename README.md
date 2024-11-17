# Videohra tower defense

Ročníková práce š. r. 2024/2025

<div>
    <img src="https://github.com/victoriousoft/tower-defense/actions/workflows/build.yml/badge.svg" alt="build status badge"/>
    <img src="https://github.com/victoriousoft/tower-defense/actions/workflows/update-submodules.yml/badge.svg" alt="submodule update status badge"/>
</div>

## Členové týmu
 - Kristián Kunc
 - Viktor Jakovec
 - Leon Kubota (Textury a animace)

## Co kde najdu?
Repozitář obsahuje videohru v Unity enginu a používá jím stanovenou strukturu souborů s následujícími doplňky:
 - Dokumentace: [`docs/`](docs/)
 - Webová stránka: [`td-web/`](https://github.com/victoriousoft/td-web/tree/main/)


## Jak spustit lokálně?
Nejjednoduší je použít Docker a docker-compose. Docker image obsahuje minimální NGINX web server, který slouží k zobrazení webové stránky.

```bash
docker-compose up
```

Webová stránka bude dostupná na adrese `http://localhost:8080/`.


**Detailnější informace o všech aspektech projektu najdete v dokumentaci.**
