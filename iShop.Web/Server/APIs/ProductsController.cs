﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using iShop.Web.Server.Commons.Extensions;
using iShop.Web.Server.Commons.Helpers;
using iShop.Web.Server.Core.Models;
using iShop.Web.Server.Core.Resources;
using iShop.Web.Server.Persistent.Repositories.Contracts;
using iShop.Web.Server.Persistent.UnitOfWork.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iShop.Web.Server.APIs
{

    [Route("/api/[controller]")]
    public class ProductsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        // GET
        [HttpGet("{id}", Name =  ApplicationConstants.ControllerName.Product)]
        public async Task<IActionResult> Get(string id)
        {
            bool isValid = Guid.TryParse(id, out var productId);
            if (!isValid)
                return InvalidId(id);

            var product = await _unitOfWork.ProductRepository.GetProduct(productId);

            if (product == null)
                NotFound(productId);

            var productResource = _mapper.Map<Product, ProductResource>(product);

            return Ok(productResource);
        }

        // GET 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.ProductRepository.GetProducts();

            var productResources = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResource>>(products);

            return Ok(productResources);
        }

        // POST
        [Authorize(Policy = ApplicationConstants.PolicyName.SuperUsers)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavedProductResource savedProductResources)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = _mapper.Map<SavedProductResource, Product>(savedProductResources);

            await _unitOfWork.ProductRepository.AddAsync(product);
            if (!await _unitOfWork.CompleteAsync())
            {
                _logger.LogMessage(LoggingEvents.SavedFail,  ApplicationConstants.ControllerName.Product, product.Id);
                return FailedToSave(product.Id);
            }

            product = await _unitOfWork.ProductRepository.GetProduct(product.Id);

            var result = _mapper.Map<Product, ProductResource>(product);

            _logger.LogMessage(LoggingEvents.Created,  ApplicationConstants.ControllerName.Product, product.Id);

            return CreatedAtRoute( ApplicationConstants.ControllerName.Product, new { id = product.Id }, result);
        }

        // PUT
        [Authorize(Policy = ApplicationConstants.PolicyName.SuperUsers)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SavedEditProductResource savedProductResource)
        {
            bool isValid = Guid.TryParse(id, out var productId);
            if (!isValid)
                return InvalidId(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _unitOfWork.ProductRepository.GetProduct(productId);

            if (product == null)
                return NotFound(productId);

            _mapper.Map(savedProductResource, product);

            if (!await _unitOfWork.CompleteAsync())
            {
                _logger.LogMessage(LoggingEvents.SavedFail,  ApplicationConstants.ControllerName.Product, product.Id);
                return FailedToSave(product.Id);
            }

            product = await _unitOfWork.ProductRepository.GetProduct(product.Id);

            var result = _mapper.Map<Product, SavedProductResource>(product);

            _logger.LogMessage(LoggingEvents.Updated,  ApplicationConstants.ControllerName.Product, product.Id);

            return Ok(result);
        }

        // DELETE
        [Authorize(Policy = ApplicationConstants.PolicyName.SuperUsers)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            bool isValid = Guid.TryParse(id, out var productId);
            if (!isValid)
                return InvalidId(id);

            var product = await _unitOfWork.ProductRepository.GetProduct(productId);

            if (product == null)
                return NotFound(productId);

            _unitOfWork.ProductRepository.Remove(product);
            if (!await _unitOfWork.CompleteAsync())
            {
                _logger.LogMessage(LoggingEvents.SavedFail,  ApplicationConstants.ControllerName.Product, product.Id);
                return FailedToSave(product.Id);
            }

            _logger.LogMessage(LoggingEvents.Deleted,  ApplicationConstants.ControllerName.Product, product.Id);


            return NoContent();
        }
    }
}