using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using DataConstants = JetBrains.ReSharper.Psi.Services.DataConstants;
using DeclaredElementUtil = JetBrains.ReSharper.Feature.Services.ExternalSources.Utils.DeclaredElementUtil;

namespace GenericOccurrencePresenter {
    public static class TypeParameterUtil {

        public static IEnumerable<IDeclaredType> GetResolvedTypeParams(IResolveResult resolution) {
            var sub = resolution.Substitution;
            var domainList = sub.Domain.ToList().OrderBy(x => x.Index);
            var typeParams = domainList.Select(x => sub.Apply(x).GetScalarType());

            return typeParams;
        }

        public static IEnumerable<IDeclaredType> GetTypeParametersFromContext(IDataContext context) {
            var reference = context.GetData(DataConstants.REFERENCE);
            if (reference == null || reference.CurrentResolveResult == null) {
                return null;
            }

            var typeParams = GetResolvedTypeParams(reference.CurrentResolveResult.Result);

            return typeParams;
        }

        public static ITypeElement GetOriginTypeElement(IDataContext dataContext,
                                                        DeclaredElementTypeUsageInfo initialTarget) {
            var data = dataContext.GetData(DataConstants.REFERENCES);
            if (data == null) {
                return null;
            }

            foreach (var current in data.OfType<IQualifiableReference>()) {
                if (!Equals(current.Resolve().DeclaredElement, initialTarget)) {
                    continue;
                }

                var qualifierWithTypeElement = current.GetQualifier() as IQualifierWithTypeElement;

                if (qualifierWithTypeElement == null) {
                    continue;
                }

                var qualifierTypeElement = qualifierWithTypeElement.GetQualifierTypeElement();
                if (qualifierTypeElement != null) {
                    return qualifierTypeElement;
                }
            }
            return null;
        }

        public static IEnumerable<IDeclaredType> GetTypeParameters(IOccurence occurence)
        {
            var element = occurence.GetDeclaredElement();
            var topLevelTypeElement = DeclaredElementUtil.GetTopLevelTypeElement(element as IClrDeclaredElement);
            var elementSuperTypes = TypeElementUtil.GetAllSuperTypesReversed(topLevelTypeElement);
            return GetTypeParametersFromTypes(elementSuperTypes).First(x => x.Value.Any()).Value;
        }
        
        public static IDeclaredType GetTypeWithTypeParameters(IOccurence occurence)
        {
            var element = occurence.GetDeclaredElement();
            var topLevelTypeElement = DeclaredElementUtil.GetTopLevelTypeElement(element as IClrDeclaredElement);
            var elementSuperTypes = TypeElementUtil.GetAllSuperTypesReversed(topLevelTypeElement);
            return GetTypeParametersFromTypes(elementSuperTypes).First(x => x.Value.Any()).Key;
        }

        private static Dictionary<IDeclaredType, IEnumerable<IDeclaredType>> GetTypeParametersFromTypes(IEnumerable<IDeclaredType> types)
        {
            return types
                .Select(x => new { Type = x, TypeParams = GetResolvedTypeParams(x.Resolve()) })
                .ToDictionary(x => x.Type, x => x.TypeParams);
        }

        public static string GetTypeText(IOccurence occurence)
        {
            var typeParameters = GetTypeParameters(occurence);
            var type = GetTypeWithTypeParameters(occurence);

            return string.Format(" {0}<{1}>", type.GetClrName().ShortName, string.Join(",", typeParameters.Select(GetName)));
        }

        private static object GetName(IDeclaredType declaredType)
        {
            var thing = declaredType.GetClrName().ShortName;
            return !string.IsNullOrEmpty(thing) ? thing : declaredType.GetPresentableName(new UnknownLanguage());
        }
    }
}