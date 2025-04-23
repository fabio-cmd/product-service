package com.servicebridge.product_service.controller;

import com.servicebridge.product_service.model.Product;
import com.servicebridge.product_service.repository.ProductRepository;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/produtos")
public class ProductController {

    private final ProductRepository repository;

    public ProductController(ProductRepository repository) {
        this.repository = repository;
    }

    @GetMapping
    public List<Product> listarTodos() {
        return repository.findAll();
    }

    @PostMapping
    public Product criar(@RequestBody Product produto) {
        return repository.save(produto);
    }

    @GetMapping("/{id}")
    public Product buscarPorId(@PathVariable Long id) {
        return repository.findById(id).orElseThrow();
    }

    @DeleteMapping("/{id}")
    public void deletar(@PathVariable Long id) {
        repository.deleteById(id);
    }
}
