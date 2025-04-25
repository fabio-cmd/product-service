package com.servicebridge.product_service;

import com.servicebridge.product_service.model.Product;      

import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.server.LocalServerPort; 
import org.springframework.test.context.ActiveProfiles;
import org.springframework.test.context.DynamicPropertyRegistry;
import org.springframework.test.context.DynamicPropertySource;

import org.springframework.http.*;         
import org.springframework.web.client.RestTemplate;

import org.testcontainers.containers.OracleContainer;
import org.testcontainers.junit.jupiter.Container;
import org.testcontainers.junit.jupiter.Testcontainers;

import static org.assertj.core.api.Assertions.assertThat;

@Testcontainers
@ActiveProfiles("tc-oracle")  
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
public class ProductApiIntegrationTests {

    @LocalServerPort
    private int port;

    private final RestTemplate restTemplate = new RestTemplate();

    @Container
    static OracleContainer oracle =
        new OracleContainer("gvenzl/oracle-xe:21-slim")
                .withReuse(true);

    @DynamicPropertySource
    static void overrideProps(DynamicPropertyRegistry r) {
        r.add("spring.datasource.url", oracle::getJdbcUrl);
        r.add("spring.datasource.username", oracle::getUsername);
        r.add("spring.datasource.password", oracle::getPassword);
    }

    @Test
    void createAndListProductSuccess() {
        String endpoint = "http://localhost:" + port + "/produtos";

        Product newProduct = new Product(null, "Monitor Full HD 24\"", "Informática", 899.90);

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Product> request = new HttpEntity<>(newProduct, headers);


        ResponseEntity<Product> postResponse = restTemplate.postForEntity(endpoint, request, Product.class);


        assertThat(postResponse.getStatusCode()).isEqualTo(HttpStatus.CREATED);
        assertThat(postResponse.getBody()).isNotNull();
        assertThat(postResponse.getBody().getNome()).isEqualTo("Monitor Full HD 24\"");
        assertThat(postResponse.getBody().getCategoria()).isEqualTo("Informática");

        ResponseEntity<Product[]> getResponse = restTemplate.getForEntity(endpoint, Product[].class);

        assertThat(getResponse.getStatusCode()).isEqualTo(HttpStatus.OK);
        assertThat(getResponse.getBody()).isNotEmpty();
        assertThat(getResponse.getBody()).anyMatch(p -> "Monitor Full HD 24\"".equals(p.getNome()));
    }

}
