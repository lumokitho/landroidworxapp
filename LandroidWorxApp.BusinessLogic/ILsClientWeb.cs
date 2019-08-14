using System;
using System.Collections.Generic;
using System.Text;

namespace LandroidWorxApp.BusinessLogic
{
    public interface ILsClientWeb
    {
        LsClientWeb_LoginResponse Login(LsClientWeb_LoginRequest request);
        LsClientWeb_GetProductsResponse GetProducts(LsClientWeb_GetProductsRequest request);
        LsClientWeb_GetProductStatusResponse GetProductStatus(LsClientWeb_GetProductStatusRequest request);
        LsClientWeb_PublishCommandResponse PublishCommand(LsClientWeb_PublishCommandRequest request);
    }
}
