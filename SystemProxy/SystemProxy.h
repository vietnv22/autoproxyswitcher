// SystemProxy.h

#pragma once

using namespace System;

namespace SystemProxy {

	public ref class ProxyConfigurator
	{
        public :void SetProxyPac(String ^pacFile);
        public :void ProxyConfigurator::SetProxy(String ^address, String ^exceptions);
        public :void ResetProxy();

		private :void RefreshIE();
	};
}
