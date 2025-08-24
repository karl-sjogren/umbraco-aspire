using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Infrastructure.Runtime.RuntimeModeValidators;

namespace Umbraco.Aspire.Umbraco.Composers;

public class RemoveHttpsValidatorComposer : IComposer {
    public void Compose(IUmbracoBuilder builder)
        => builder.RuntimeModeValidators().Remove<UseHttpsValidator>();
}
