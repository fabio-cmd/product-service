package com.servicebridge.product_service.controller;

import com.servicebridge.product_service.model.Product;
import com.servicebridge.product_service.repository.ProductRepository;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.net.URI;
import java.util.List;

@RestController
@RequestMapping("/produtos")
@RequiredArgsConstructor          
public class ProductController {

    private final ProductRepository repository;

    @PostMapping
    public ResponseEntity<Product> create(@RequestBody @Valid Product dto) {
        Product saved = repository.save(dto);
        URI location = URI.create("/produtos/" + saved.getId());
        return ResponseEntity.created(location).body(saved);
    }

    @GetMapping
    public List<Product> listarTodos() {
        return repository.findAll();
    }

  
    @GetMapping("/{id}")
    public Product buscarPorId(@PathVariable Long id) {
        return repository.findById(id).orElseThrow();   
    }

   
    @DeleteMapping("/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void deletar(@PathVariable Long id) {
        repository.deleteById(id);
    }
}
