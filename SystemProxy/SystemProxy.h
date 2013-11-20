// SystemProxy.h

#pragma once

using namespace System;

namespace SystemProxy {

	public ref class ProxyConfigurator
	{
	private:
		static log4net::ILog^ log = log4net::LogManager::GetLogger("SystemProxy");

    public:
		void SetProxyPac(String ^pacFile);
		void ProxyConfigurator::SetProxy(String ^address, String ^exceptions);
		void ResetProxy();

	private:
		void Refresh();
	};
}
