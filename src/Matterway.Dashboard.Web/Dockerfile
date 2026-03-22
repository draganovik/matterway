# syntax=docker/dockerfile:1.7
ARG NODE_VERSION=node:20.19.0

FROM ${NODE_VERSION} AS base
WORKDIR /app

FROM base AS deps
COPY package.json package-lock.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm ci --ignore-scripts --no-audit --no-fund

FROM deps AS build
COPY . .
RUN npm run postinstall \
    && npm run build

FROM ${NODE_VERSION}-slim AS runtime
WORKDIR /app

ENV NODE_ENV=production
ENV NUXT_HOST=0.0.0.0
ARG NUXT_APP_VERSION
ENV NUXT_APP_VERSION=${NUXT_APP_VERSION}

COPY --from=build --chown=node:node /app/.output /app/.output

USER node
CMD [ "node", "/app/.output/server/index.mjs" ]
