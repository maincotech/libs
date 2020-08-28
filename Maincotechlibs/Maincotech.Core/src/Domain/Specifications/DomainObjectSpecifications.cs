using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Maincotech.Domain.Specifications
{
    public static class DomainObjectSpecifications
    {
        public static ISpecification<TDomainObject> Id<TDomainObject>(Guid id) where TDomainObject : DomainObject
        {
            return Specification<TDomainObject>.Eval(entity => entity.Id == id);
        }

        public static ISpecification<TDomainObject> ChangedAfter<TDomainObject>(DateTime lastModifiedTime) where TDomainObject : DomainObject
        {
            return Specification<TDomainObject>.Eval(entity => entity.LastModifiedTime > lastModifiedTime);
        }

        public static ISpecification<TDomainObject> IdIn<TDomainObject>(List<Guid> ids) where TDomainObject : DomainObject
        {
            var firstId = ids[0];
            ISpecification<TDomainObject> specification =
                Specification<TDomainObject>.Eval(entity => entity.Id == firstId);
            for (var i = 1; i < ids.Count; i++)
            {
                var currentId = ids[i];
                specification = specification.Or(Specification<TDomainObject>.Eval(entity => entity.Id == currentId));
            }
            return specification;
        }

        //public static ISpecification<T> SourceIdIn<T>(List<Guid> ids) where T: IAssociation
        //{
        //    ISpecification<Association> specification =
        //        Specification<Association>.Eval(entity => entity.SourceId == ids[0]);
        //    for (var i = 1; i < ids.Count; i++)
        //    {
        //        specification = specification.Or(Specification<Association>.Eval(entity => entity.SourceId == ids[i]));
        //    }
        //    return specification;
        //}

        //public static ISpecification<Association> TargetIdIn(List<Guid> ids)
        //{
        //    ISpecification<Association> specification =
        //        Specification<Association>.Eval(entity => entity.SourceId == ids[0]);
        //    for (var i = 1; i < ids.Count; i++)
        //    {
        //        specification = specification.Or(Specification<Association>.Eval(entity => entity.TargetId == ids[i]));
        //    }
        //    return specification;
        //}

        //public static ISpecification<Association> NameEqual(string name)
        //{
        //    ISpecification<Association> specification = Specification<Association>.Eval(entity => entity.Name == name);
        //    return specification;
        //}
    }
}