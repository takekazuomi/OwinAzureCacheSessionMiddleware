OWIN Azure Cache Session Middleware
====================================
SessionをAzure Cache Serviceに保存するOWIN Middlewareのサンプルです。

サンプルアプリ
==============
SelfHostSample にOWin Selft Host でMiddlewareを動かすサンプルアプリが入っています。

設定
====
App.config.template 内の configuration/dataCacheClients/dataCacheClient/autoDiscover のidentifier属性をAzure CacheのEndpointにして、securityProperties/messageSecurityのauthorizationInfo属性にManage Access Keys を設定します。
