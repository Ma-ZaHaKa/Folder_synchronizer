# Folder_synchronizer
PHP&lt;-C# folder synchronizer
Приложение для синхронизации локальной папки с сервером по HTTP

Настройка
1. Загрузить php-shell на сервер из bin/Release/controller.php
Пример: http://mydomain.com/ROOTDIR/controller/controller.php
Корень синхронизации -> http://mydomain.com/ROOTDIR/

2. Настроить Config.json рядом с SYNC_DIR.exe
 2.1 controller_api_url (STRING) -> Путь к PHP-Shell. Пример: "http://mydomain.com/ROOTDIR/controller/controller.php"
 2.2 controller_root_path (STRING) -> Путь к корню папки синхронизаци. Пример: папка ROOTDIR, нужно перейти от shell-а в папку синхронизации, "../"
 2.3 local_sync_dir (STRING) -> Путь к локальной папке синхронизации. Пример: "D:\\ROOTDIR\\"
 2.4 always_rewrite_all (BOOL) -> Флаг отвечающий за колличество загрузаемых файлов. Пример: true-> всегда загружает всё содержимое/ false-> Ищет разницу в файлах и обновляет её
 2.5 clear_api_sync_dir (BOOL) -> Флаг отвечающий за очищение папки при полной загрузке всех файлов. Пример: true -> очистит серверную папку при первой синхронизации или всегда с включённым флагом always_rewrite_all (Режим полное обновление)/ false-> если на сервере присутствует папка с именем локальной папки, очистки не будет, файлы дозапишуться к ней
 2.6 hash_history_file (STRING) -> Имя файла, хранящий историю загруженных в крайний раз файлов, для поиска разницы версий
 2.7 sync_name_api_dir (BOOL) -> Флаг отвечающий за создание папки на сервере с именем локальной папки. Тут 2 варианта, первый - использовать корень "../" и включить sync_name_api_dir, второй - первый - использовать корень "../ROOTDIR/" и выключить sync_name_api_dir, результат будет одинаковый -> "http://mydomain.com/ROOTDIR/synclocaldirsandfiles". Пример: true -> "http://mydomain.com/ROOTDIR/ROOTDIR/synclocaldirsandfiles" / false -> "http://mydomain.com/ROOTDIR/synclocaldirsandfiles"
 
3. Запустить SYNC_DIR.exe
 
4. Готово
