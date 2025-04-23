FROM maven:3.9.3-eclipse-temurin-17 AS builder
WORKDIR /app
COPY api-java .
RUN mvn clean package -DskipTests

FROM eclipse-temurin:17-jdk-alpine
WORKDIR /app

RUN apk add --no-cache bash coreutils


COPY --from=builder /app/target/product-service-0.0.1-SNAPSHOT.jar app.jar
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

EXPOSE 8080


ENTRYPOINT ["/wait-for-it.sh", "oracle-db:1521", "--", "java", "-jar", "app.jar"]
