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
import org.springframework.web.client.HttpClientErrorException;


import org.testcontainers.containers.OracleContainer;
import org.testcontainers.junit.jupiter.Container;
import org.testcontainers.junit.jupiter.Testcontainers;

import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.fail;


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

    @Test
    void updateProductSuccess() {

        String endpoint = "http://localhost:" + port + "/produtos";
        Product original = new Product(null, "Mouse", "Informática", 129.90);
        Product created = restTemplate.postForObject(endpoint, original, Product.class);
        assertThat(created.getId()).isNotNull();


        Product atualizado = new Product(created.getId(), "Mouse Gamer", "Informática", 199.90);

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Product> req = new HttpEntity<>(atualizado, headers);
        ResponseEntity<Product> putResp = restTemplate.exchange(
            endpoint + "/" + created.getId(), HttpMethod.PUT, req, Product.class);

        assertThat(putResp.getStatusCode()).isEqualTo(HttpStatus.OK);
        assertThat(putResp.getBody().getNome()).isEqualTo("Mouse Gamer");
        assertThat(putResp.getBody().getPreco()).isEqualTo(199.90);
}

    @Test
    void deleteProductSuccess() {

        String endpoint = "http://localhost:" + port + "/produtos";

        Product newProduct = new Product(null, "Teclado", "Informática", 249.90);
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        HttpEntity<Product> request = new HttpEntity<>(newProduct, headers);
        ResponseEntity<Product> postResponse = restTemplate.postForEntity(endpoint, request, Product.class);
        assertThat(postResponse.getStatusCode()).isEqualTo(HttpStatus.CREATED);
    

    Long id = postResponse.getBody().getId();

    restTemplate.delete(endpoint + "/" + id);

    try {
        restTemplate.getForEntity(endpoint + "/" + id, Product.class);
        fail("HttpClientErrorException.NotFound");
    } catch (HttpClientErrorException.NotFound ex) {
        assertThat(ex.getStatusCode()).isEqualTo(HttpStatus.NOT_FOUND);
    }

    }

}
