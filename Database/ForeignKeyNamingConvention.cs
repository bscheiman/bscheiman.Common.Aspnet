#region
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

#endregion

namespace bscheiman.Common.Aspnet.Database {
    public class ForeignKeyNamingConvention : IStoreModelConvention<AssociationType> {
        public void Apply(AssociationType association, DbModel model) {
            if (!association.IsForeignKey)
                return;

            var constraint = association.Constraint;

            if (DoPropertiesHaveDefaultNames(constraint.FromProperties, constraint.ToProperties))
                NormalizeForeignKeyProperties(constraint.FromProperties);

            if (DoPropertiesHaveDefaultNames(constraint.ToProperties, constraint.FromProperties))
                NormalizeForeignKeyProperties(constraint.ToProperties);
        }

        private bool DoPropertiesHaveDefaultNames(ReadOnlyMetadataCollection<EdmProperty> properties,
            ReadOnlyMetadataCollection<EdmProperty> otherEndProperties) {
            if (properties.Count == otherEndProperties.Count) {
                for (int i = 0; i < properties.Count; ++i) {
                    if (properties[i].Name.EndsWith("_" + otherEndProperties[i].Name))
                        return true;

                    var preferredNameProperty = otherEndProperties[i].MetadataProperties.SingleOrDefault(x => x.Name.Equals("PreferredName"));

                    if (preferredNameProperty == null)
                        continue;

                    if (properties[i].Name.EndsWith("_" + preferredNameProperty.Value))
                        return true;
                }
            }

            return false;
        }

        private void NormalizeForeignKeyProperties(ReadOnlyMetadataCollection<EdmProperty> properties) {
            foreach (var t in properties) {
                string defaultPropertyName = t.Name;
                int ichUnderscore = defaultPropertyName.IndexOf('_');

                if (ichUnderscore <= 0)
                    continue;

                string navigationPropertyName = defaultPropertyName.Substring(0, ichUnderscore);
                string targetKey = defaultPropertyName.Substring(ichUnderscore + 1);

                string newPropertyName;

                if (targetKey.StartsWith(navigationPropertyName))
                    newPropertyName = targetKey;
                else
                    newPropertyName = navigationPropertyName + targetKey;

                t.Name = newPropertyName;
            }
        }
    }
}