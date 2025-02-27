using System;
using System.Numerics;
using System.Reactive;
using System.Threading.Tasks;
using Haus.Core.Models.Lighting;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Shared.Lighting;

public class LightingViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedWithoutLightingThenShowsNoLighting()
    {
        var view = RenderLighting(null);

        view.FindByClass("mud-paper").TextContent.Should().Contain("no lighting");
    }

    [Fact]
    public void WhenRenderedThenLevelSliderIsLimitedByLighting()
    {
        var lighting = HausModelFactory.LightingModel() with
        {
            Level = new LevelLightingModel(Min: 20, Value: 60, Max: 80),
        };
        var view = RenderLighting(lighting);

        var level = FindSliderByClass<double>(view, "level");

        level.Instance.Value.Should().Be(60);
        level.Instance.Min.Should().Be(20);
        level.Instance.Max.Should().Be(80);
    }

    [Fact]
    public void WhenRenderedWithTemperatureThenTemperatureSliderIsLimitedByTemperature()
    {
        var lighting = HausModelFactory.LightingModel() with
        {
            Temperature = new TemperatureLightingModel(Min: 2000, Value: 4700, Max: 6500),
        };
        var view = RenderLighting(lighting);

        var temp = FindSliderByClass<double>(view, "temperature");
        temp.Instance.Min.Should().Be(2000);
        temp.Instance.Value.Should().Be(4700);
        temp.Instance.Max.Should().Be(6500);
    }

    [Fact]
    public void WhenRenderedWithColorThenRedSliderIsAvailable()
    {
        var lighting = HausModelFactory.LightingModel() with { Color = new ColorLightingModel(Red: 100) };
        var view = RenderLighting(lighting);

        var red = FindSliderByClass<byte>(view, "red");
        red.Instance.Min.Should().Be(0);
        red.Instance.Value.Should().Be(100);
        red.Instance.Max.Should().Be(255);
    }

    [Fact]
    public void WhenRenderedWithColorThenGreenSliderIsAvailable()
    {
        var lighting = HausModelFactory.LightingModel() with { Color = new ColorLightingModel(Green: 50) };
        var view = RenderLighting(lighting);

        var green = FindSliderByClass<byte>(view, "green");
        green.Instance.Min.Should().Be(0);
        green.Instance.Value.Should().Be(50);
        green.Instance.Max.Should().Be(255);
    }

    [Fact]
    public void WhenRenderedWithColorThenBlueSliderIsAvailable()
    {
        var lighting = HausModelFactory.LightingModel() with { Color = new ColorLightingModel(Blue: 150) };
        var view = RenderLighting(lighting);

        var blue = FindSliderByClass<byte>(view, "blue");
        blue.Instance.Min.Should().Be(0);
        blue.Instance.Value.Should().Be(150);
        blue.Instance.Max.Should().Be(255);
    }

    [Fact]
    public async Task WhenLevelIsAdjustedThenNotifiesLightingChanged()
    {
        var lighting = HausModelFactory.LightingModel();
        LightingModel? changed = null;
        var view = RenderLighting(lighting, l => changed = l);

        await view.InvokeAsync(async () =>
        {
            await FindSliderByClass<double>(view, "level").Instance.ValueChanged.InvokeAsync(40);
        });

        Eventually.Assert(() =>
        {
            changed?.Level.Value.Should().Be(40);
        });
    }

    [Fact]
    public async Task WhenTemperatureIsAdjustedThenNotifiesTemperatureChanged()
    {
        var lighting = HausModelFactory.LightingModel() with
        {
            Temperature = new TemperatureLightingModel(Min: 2000, Value: 3000, Max: 9000),
        };
        LightingModel? changed = null;
        var view = RenderLighting(lighting, l => changed = l);

        await view.InvokeAsync(async () =>
        {
            await FindSliderByClass<double>(view, "temperature").Instance.ValueChanged.InvokeAsync(6000);
        });

        Eventually.Assert(() =>
        {
            changed?.Temperature?.Value.Should().Be(6000);
        });
    }

    [Fact]
    public async Task WhenLevelIsAdjustedRapidlyThenNotifiesOnce()
    {
        var lighting = HausModelFactory.LightingModel();
        var timesChanged = 0;
        var view = RenderLighting(lighting, _ => timesChanged++);

        await view.InvokeAsync(async () =>
        {
            await FindSliderByClass<double>(view, "level").Instance.ValueChanged.InvokeAsync(40);
            await FindSliderByClass<double>(view, "level").Instance.ValueChanged.InvokeAsync(42);
            await FindSliderByClass<double>(view, "level").Instance.ValueChanged.InvokeAsync(44);
            await FindSliderByClass<double>(view, "level").Instance.ValueChanged.InvokeAsync(46);
            await Task.Delay(250);
        });

        timesChanged.Should().Be(1);
    }

    [Fact]
    public async Task WhenLightingIsAdjustedRapidlyThenNotifiesOnceThereIsAPause()
    {
        var lighting = HausModelFactory.LightingModel() with
        {
            Temperature = new TemperatureLightingModel(Min: 2000, Value: 3000, Max: 9000),
            Color = new ColorLightingModel(Red: 100, Green: 100, Blue: 100),
        };
        var timesChanged = 0;
        var view = RenderLighting(lighting, _ => timesChanged++);

        await view.InvokeAsync(async () =>
        {
            await FindSliderByClass<double>(view, "level").Instance.ValueChanged.InvokeAsync(40);
            await FindSliderByClass<double>(view, "temperature").Instance.ValueChanged.InvokeAsync(4000);
            await FindSliderByClass<byte>(view, "red").Instance.ValueChanged.InvokeAsync(44);
            await FindSliderByClass<byte>(view, "green").Instance.ValueChanged.InvokeAsync(46);
            await FindSliderByClass<byte>(view, "blue").Instance.ValueChanged.InvokeAsync(46);
            await Task.Delay(250);
        });

        timesChanged.Should().Be(1);
    }

    private IRenderedComponent<LightingView> RenderLighting(
        LightingModel? lighting,
        Action<LightingModel>? onChanged = null
    )
    {
        return Context.RenderComponent<LightingView>(opts =>
        {
            opts.Add(o => o.Lighting, lighting);
            if (onChanged != null)
            {
                opts.Add(o => o.OnLightingChanged, onChanged);
            }
        });
    }

    private static IRenderedComponent<MudSlider<T>> FindSliderByClass<T>(
        IRenderedComponent<LightingView> view,
        string className
    )
        where T : struct, INumber<T>
    {
        return view.FindByComponent<MudSlider<T>>(opts => opts.WithClassName(className));
    }
}
