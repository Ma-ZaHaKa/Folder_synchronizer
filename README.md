# Folder_synchronizer
PHP&lt;-C# folder synchronizer
Приложение для синхронизации локальной папки с сервером по HTTP

Настройка
1. Загрузить php-shell на сервер из bin/Release/controller.php
Пример: http://mydomain.com/ROOTDIR/controller/controller.php
Корень синхронизации -> http://mydomain.com/ROOTDIR/

2. Настроить Config.json рядом с SYNC_DIR.exe
3. Параметр "controller_api_url" (STRING) -> Путь к PHP-Shell. Пример: "http://mydomain.com/ROOTDIR/controller/controller.php"
4. Параметр "controller_root_path" (STRING) -> Путь к корню серверной папки синхронизаци. Пример: папка ROOTDIR, нужно перейти от shell-а в папку синхронизации, "../"
5. Параметр "local_sync_dir" (STRING) -> Путь к локальной папке синхронизации. Пример: "D:\\\ROOTDIR\\\\"
6. Параметр "always_rewrite_all" (BOOL) -> Флаг отвечающий за колличество загрузаемых файлов. Пример: true-> всегда загружает всё содержимое/ false-> Ищет разницу в файлах и обновляет её
7. Параметр "clear_api_sync_dir" (BOOL) -> Флаг отвечающий за очищение папки при полной загрузке всех файлов. Пример: true -> очистит серверную папку при первой синхронизации или всегда с включённым флагом always_rewrite_all (Режим полное обновление)/ false-> если на сервере присутствует папка с именем локальной папки, очистки не будет, файлы дозапишуться к ней
8. Параметр "hash_history_file" (STRING) -> Имя файла, хранящий историю загруженных в крайний раз файлов, для поиска разницы версий. Пример: "hash.json"
9. Параметр "sync_name_api_dir" (BOOL) -> Флаг отвечающий за создание папки на сервере с именем локальной папки. Тут 2 варианта, первый - использовать корень "../" и включить sync_name_api_dir, второй - использовать корень "../ROOTDIR/" и выключить sync_name_api_dir, результат будет одинаковый -> "http://mydomain.com/ROOTDIR/synclocaldirsandfiles". Пример: true -> "http://mydomain.com/ROOTDIR/ROOTDIR/synclocaldirsandfiles" / false -> "http://mydomain.com/ROOTDIR/synclocaldirsandfiles"
 
 
10. Запустить SYNC_DIR.exe (Запуск приложения синхронизирует локальную папку с серверной. Он не работает в фоне, 1 запуск 1 синхронизация.)
 
11. Готово
