FROM gradle:6.8.3-jdk8 AS build
COPY --chown=gradle:gradle . /home/gradle/eureka-server
WORKDIR /home/gradle/eureka-server
RUN gradle build --no-daemon

FROM eclipse-temurin:8-jre

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 8761

RUN mkdir /app

COPY --from=build /home/gradle/eureka-server/build/libs/eureka-server-0.0.1-SNAPSHOT.jar /app/eureka-server.jar

ENTRYPOINT ["java","-jar","/app/eureka-server.jar", "--server.port=8761"]