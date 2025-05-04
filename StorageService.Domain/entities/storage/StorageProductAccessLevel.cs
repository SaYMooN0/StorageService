namespace StorageService.Domain.entities.storage;

public enum StorageProductAccessLevel
{
    OnlyOwner,        //только админ хранилища
    Authenticated,    //любой админ, то есть авторизованный пользователь
    Public            //все, включая неавторизованных
}