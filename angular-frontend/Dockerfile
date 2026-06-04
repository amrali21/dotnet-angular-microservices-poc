FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
# Which Angular build configuration to bake in. Defaults to "docker" so the
# existing docker-compose build is unchanged. For Kubernetes build with:
#   docker build --build-arg NG_CONFIG=kubernetes -t angular-frontend:k8s-1 .
# (kubernetes config sets ACTION_URL='' -> relative, same-origin API calls.)
ARG NG_CONFIG=docker
RUN npx ng build --configuration $NG_CONFIG

FROM nginx:alpine
COPY --from=build /app/dist/test-angular/browser /usr/share/nginx/html
COPY nginx.docker.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
