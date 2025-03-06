FROM nginx:alpine

WORKDIR /etc/nginx/conf.d
COPY nginx.conf default.conf

WORKDIR /etc/nginx/html
COPY Build/WebGL/WebGL .

RUN mkdir -p ./static
COPY balancer/out/combined.csv ./static/balancer-data.csv

CMD ["nginx", "-g", "daemon off;"]