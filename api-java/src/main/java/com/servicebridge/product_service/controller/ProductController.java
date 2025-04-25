package com.servicebridge.product_service.controller;

import com.servicebridge.product_service.model.Product;
import com.servicebridge.product_service.repository.ProductRepository;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import org.springframework.web.server.ResponseStatusException;


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
    public Product searchId(@PathVariable Long id) {
        return repository.findById(id)
        .orElseThrow(()-> new ResponseStatusException(HttpStatus.NOT_FOUND, "Produto não encontrado"));   
    }

    @PutMapping("/{id}")
    public ResponseEntity<Product> atualizar(@PathVariable Long id,
                                         @RequestBody @Valid Product dto) {
    Product saved = repository.findById(id)
                              .map(p -> {
                                  p.setNome(dto.getNome());
                                  p.setCategoria(dto.getCategoria());
                                  p.setPreco(dto.getPreco());
                                  return repository.save(p);
                              }).orElseThrow();
    return ResponseEntity.ok(saved);
}


   
    @DeleteMapping("/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void deletar(@PathVariable Long id) {
        repository.deleteById(id);
    }
}
