using CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using CleanArchitecture.Domain.Entities.ExtendedAttributes;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Server.Controllers.Utilities.ExtendedAttributes.Base;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.Utilities.ExtendedAttributes.Sgcd
{
    public class DocumentExtendedAttributesController : ExtendedAttributesController<Guid, Guid, Document, DocumentExtendedAttribute>
    {
        [Authorize(Policy = Permissions.DocumentExtendedAttributes.View)]
        public override Task<IActionResult> GetAll()
        {
            return base.GetAll();
        }

        [Authorize(Policy = Permissions.DocumentExtendedAttributes.View)]
        public override Task<IActionResult> GetAllByEntityId(Guid entityId)
        {
            return base.GetAllByEntityId(entityId);
        }

        [Authorize(Policy = Permissions.DocumentExtendedAttributes.View)]
        public override Task<IActionResult> GetById(Guid id)
        {
            return base.GetById(id);
        }

        [Authorize(Policy = Permissions.DocumentExtendedAttributes.Create)]
        public override Task<IActionResult> Post(AddEditExtendedAttributeCommand<Guid, Guid, Document, DocumentExtendedAttribute> command)
        {
            return base.Post(command);
        }

        [Authorize(Policy = Permissions.DocumentExtendedAttributes.Delete)]
        public override Task<IActionResult> Delete(Guid id)
        {
            return base.Delete(id);
        }

        [Authorize(Policy = Permissions.DocumentExtendedAttributes.Export)]
        public override Task<IActionResult> Export(string searchString = "", Guid entityId = default, bool includeEntity = false, bool onlyCurrentGroup = false, string currentGroup = "")
        {
            return base.Export(searchString, entityId, includeEntity, onlyCurrentGroup, currentGroup);
        }
    }
}