using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using Presentation.ActionFilters;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Presentation.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParamaters bookParameters)
        {
            var pagedResult =await _manager
                .BookService
                .GetAllBooksAsync(bookParameters,false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

            return Ok(pagedResult.books);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var book =await _manager
            .BookService
            .GetOneBookByIdAsync(id, false);

            
            return Ok(book); 
        }


        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {         
            var book=await _manager.BookService.CreateOneBookASync(bookDto);
            return StatusCode(201, book);          
        }


        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto )
        {
            //if (bookDto is null) Validation will check that two state
            //{
            //    return BadRequest();
            //}
            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);

            await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);
            return NoContent();           
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            await _manager.BookService.DeleteOneBookAsync(id, false);
            return NoContent();      
        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
            {
                return BadRequest();
            }
            var result = _manager.BookService.GetOneBookForPatchAsync(id, false);
            
            bookPatch.ApplyTo(result.Result.bookDtoForUpdate,ModelState);

            TryValidateModel(result.Result.bookDtoForUpdate);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            await _manager.BookService.SaveChangesForPatchAsync(result.Result.bookDtoForUpdate, result.Result.book);

            return NoContent(); //204
        }
    }

}
