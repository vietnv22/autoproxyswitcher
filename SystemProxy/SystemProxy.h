// SystemProxy.h

#pragma once

using namespace System;

namespace SystemProxy {

	public ref class ProxyConfigurator
	{
        public :void SetProxyPac(String ^pacFile);
        public :void ProxyConfigurator::SetProxy(String ^address);
        public :void ResetProxy();
	};
}
