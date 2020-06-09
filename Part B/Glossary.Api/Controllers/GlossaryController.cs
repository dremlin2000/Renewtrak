using AutoMapper;
using Glossary.Core.Abstract.Repositories;
using Glossary.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Glossary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlossaryController : ControllerBase
    {
        private readonly IGlossaryRepository _glossaryRepository;
        private readonly IMapper _mapper;

        public GlossaryController(
            IGlossaryRepository glossaryRepository,
            IMapper mapper)
        {
            _glossaryRepository = glossaryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GlossaryVm>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var glossaryItems = await _glossaryRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<GlossaryVm>>(glossaryItems));
        }

        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType(typeof(GlossaryVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorMessageVm), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var glossaryItem = await _glossaryRepository.GetAsync(id);

            if (glossaryItem == null)
            {
                return NotFound("The requested glossary term has not been found.");
            }

            return Ok(_mapper.Map<GlossaryVm>(glossaryItem));
        }

        [HttpPost]
        [ProducesResponseType(typeof(GlossaryVm), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorMessageVm), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Add(GlossaryVm glossaryVm)
        {
            var glossaryModel = _mapper.Map<Core.Models.Glossary>(glossaryVm);

            _glossaryRepository.Add(glossaryModel);
            await _glossaryRepository.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetById), new { glossaryVm.Id }, glossaryVm);
        }

        [HttpPut]
        [ProducesResponseType(typeof(GlossaryVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorMessageVm), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(GlossaryVm glossaryVm)
        {
            var getByIdResult = await GetById(glossaryVm.Id);

            if (!(getByIdResult is OkObjectResult))
            {
                return getByIdResult;
            }

            var glossaryModel = _mapper.Map<Core.Models.Glossary>(glossaryVm);

            _glossaryRepository.Update(glossaryModel);

            await _glossaryRepository.SaveChangesAsync();

            return Ok(glossaryVm);
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorMessageVm), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var getByIdResult = await GetById(id);

            if (!(getByIdResult is OkObjectResult))
            {
                return getByIdResult;
            }

            await _glossaryRepository.DeleteAsync(id);
            await _glossaryRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
