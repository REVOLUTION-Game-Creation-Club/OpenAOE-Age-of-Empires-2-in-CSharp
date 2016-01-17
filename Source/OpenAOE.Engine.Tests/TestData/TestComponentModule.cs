﻿using Ninject.Modules;
using OpenAOE.Engine.Tests.TestData.Components;

namespace OpenAOE.Engine.Tests.TestData
{
    class TestComponentModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISimpleComponent>().To<SimpleComponent>();
            Bind<IOtherSimpleComponent>().To<OtherSimpleComponent>();
        }
    }
}
