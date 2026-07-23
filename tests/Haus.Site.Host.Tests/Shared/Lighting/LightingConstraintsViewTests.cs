using System;
using System.Threading.Tasks;
using Haus.Core.Models.Lighting;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;

namespace Haus.Site.Host.Tests.Shared.Lighting;

public class LightingConstraintsViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsConstraintsValues()
    {
        var model = new LightingConstraintsModel(
            MinLevel: 20,
            MaxLevel: 80,
            MinTemperature: 4000,
            MaxTemperature: 8000
        );

        var view = RenderConstraints(model);

        Assert.Equal(20, view.FindMudTextFieldById<double>("minLevel").GetValue());
        Assert.Equal(80, view.FindMudTextFieldById<double>("maxLevel").GetValue());
        Assert.Equal(4000, view.FindMudTextFieldById<double?>("minTemperature").GetValue());
        Assert.Equal(8000, view.FindMudTextFieldById<double?>("maxTemperature").GetValue());
    }

    [Fact]
    public async Task WhenMinLevelIsModifiedThenNotifiesConstraintsChanged()
    {
        var model = HausModelFactory.LightingConstraintsModel() with { MinLevel = 10 };

        LightingConstraintsModel? updated = null;
        var view = RenderConstraints(model, c => updated = c);

        await ChangeInputValue<double>(view, "minLevel", 30);

        Eventually.Assert(() =>
        {
            Assert.Equal(model with { MinLevel = 30 }, updated);
        });
    }

    [Fact]
    public async Task WhenMaxLevelIsModifiedThenNotifiesConstraintsChanged()
    {
        var model = HausModelFactory.LightingConstraintsModel() with { MaxLevel = 90 };

        LightingConstraintsModel? updated = null;
        var view = RenderConstraints(model, c => updated = c);

        await ChangeInputValue<double>(view, "maxLevel", 80);

        Eventually.Assert(() =>
        {
            Assert.Equal(model with { MaxLevel = 80 }, updated);
        });
    }

    [Fact]
    public async Task WhenMinTemperatureIsModifiedThenNotifiesConstraintsChanged()
    {
        var model = HausModelFactory.LightingConstraintsModel() with { MinTemperature = 2000 };

        LightingConstraintsModel? updated = null;
        var view = RenderConstraints(model, c => updated = c);

        await ChangeInputValue<double?>(view, "minTemperature", 4000);

        Eventually.Assert(() =>
        {
            Assert.Equal(model with { MinTemperature = 4000 }, updated);
        });
    }

    [Fact]
    public async Task WhenMaxTemperatureIsModifiedThenNotifiesConstraintsChanged()
    {
        var model = HausModelFactory.LightingConstraintsModel() with { MaxTemperature = 9000 };

        LightingConstraintsModel? updated = null;
        var view = RenderConstraints(model, c => updated = c);

        await ChangeInputValue<double?>(view, "maxTemperature", 7000);

        Eventually.Assert(() =>
        {
            Assert.Equal(model with { MaxTemperature = 7000 }, updated);
        });
    }

    private IRenderedComponent<LightingConstraintsView> RenderConstraints(
        LightingConstraintsModel model,
        Action<LightingConstraintsModel>? onChanged = null
    )
    {
        return RenderView<LightingConstraintsView>(opts =>
        {
            opts.Add(c => c.Constraints, model);
            if (onChanged != null)
            {
                opts.Add(c => c.OnChanged, onChanged);
            }
        });
    }

    private async Task ChangeInputValue<T>(IRenderedComponent<LightingConstraintsView> view, string inputId, T value)
    {
        await view.InvokeAsync(async () =>
        {
            await view.FindMudTextFieldById<T>(inputId).Instance.SetTextAsync($"{value}");
        });
    }
}
