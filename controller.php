<?php //-----BY DIKTOR
	//error_reporting(1);
	$file_id = 'file'; // form name, имя файла загрузки
	$root = $_GET['root']; //'./../'; // MAIN DIR, от кого отталкиваеться путь к файлу
	if(!isset($_GET['root'])) { echo "ROOT DIR NOT SET"; return; }
	$file = $_GET['file']; // FILE + PATH ABOVE dk_proj
	$action = $_GET['action'];
	
	
	
	
	//-------------------------------UPLOAD--FILE-----
	if ((isset($action)) && ($action == 'upload') && (!empty($_FILES[$file_id]["name"])) && isset($file)) // action + action + fileadded from  + path to upload
	{
		if($file[(strlen($file)-1)] != '/') { $file = ($file."/"); }
		$uploaddir = getcwd(). '/uploads/'; // current dir
		$uploaddir = $root.$file;
		if(!is_dir($uploaddir)) { mkdir($uploaddir, 0777, true); }
		$uploadfile = $uploaddir . basename($_FILES[$file_id]['name']); //file name input
		if (move_uploaded_file($_FILES[$file_id]['tmp_name'], $uploadfile)) {  //file name input
		    echo "UPLOAD";
		} else {
		    echo "ERROR UPLOAD";
		}
		return;
	}
	else if ((isset($action)) && ($action == 'upload') && ((empty($_FILES[$file_id]["name"])) || (!isset($file)))) 
	{
		echo isset($file) ? "FILE NOT ADDED TO FORM" : "PATH NOT SET (file=123/)";
		return;
	}
	
	//-------------------------------REMOVE--FILE---------
	else if ((isset($action)) && ($action == 'rmfile') && isset($file)) // action + action + fileadded from  + path to upload
	{
		$rmf = $root.$file;
		if(!is_file($rmf)) { echo "RMFILE FALSE! FILE NOT EXISTS!"; return; }
		unlink($rmf);
		echo is_file($rmf) ? "RMFILE ERROR" : "RMFILE OK";
		return;
	}
	else if ((isset($action)) && ($action == 'rmfile') && (!isset($file))) 
	{
		echo "RMFILE PATH NOT SET";
		return;
	}
	
	//----------------------------HASH--FILE----------
	else if((isset($action)) && ($action == 'hash') && (isset($file)))
	{
		if(file_exists($root.$file) && (!is_dir($root.$file))) { echo strtoupper(md5_file($root.$file)); } else { echo "File not found"; } return;
	}
	else if((isset($action)) && ($action == 'hash') && (!isset($file)))
	{
		echo "FILE FOR HASH NOT SET"; return;
	}
	
	//--------------------------CHECK--FILE--------
	else if((isset($action)) && ($action == 'check') && (isset($file)))
	{
		$isfile = (file_exists($root.$file) && (!is_dir($root.$file))); echo $isfile ? "EXIST" : "NO EXIST"; return;
	}
	else if((isset($action)) && ($action == 'check') && (!isset($file)))
	{
		echo "FILE FOR CHECK NOT SET"; return;
	}
	
	
	
	
	
	
	//-------------------------------MKDIR-----------
	else if ((isset($action)) && ($action == 'mkdir') && isset($file)) // action + action + fileadded from  + path to upload
	{
		$uploaddir = $root.$file;
		mkdir($uploaddir, 0777, true); //path rwx recursive
		echo "MKDIR OK";
		return;
	}
	else if ((isset($action)) && ($action == 'mkdir') && (!isset($file))) 
	{
		echo "MKDIR PATH NOT SET";
		return;
	}
	
	//-------------------------------RMDIR----------
	else if ((isset($action)) && ($action == 'rmdir') && isset($file)) // action + action + fileadded from  + path to upload
	{
		if(!is_dir($root.$file)) { echo "RMDIR FALSE! DIR NOT EXISTS!"; return; }
		$ddir = $root.$file;
		deleteDir($ddir);
		echo "RMDIR OK";
		return;
	}
	else if ((isset($action)) && ($action == 'rmdir') && (!isset($file))) 
	{
		echo "RMDIR PATH NOT SET";
		return;
	}
	
	//-------------------------------CLSDIR----------
	else if ((isset($action)) && ($action == 'clsdir') && isset($file)) // action + action + fileadded from  + path to upload
	{
		if(!is_dir($root.$file)) { echo "CLSDIR FALSE! DIR NOT EXISTS!"; return; }
		$ddir = $root.$file;
		clearDir($ddir);
		echo "CLSDIR OK";
		return;
	}
	else if ((isset($action)) && ($action == 'clsdir') && (!isset($file))) 
	{
		echo "CLSDIR PATH NOT SET";
		return;
	}
	
	//--------------------------CHECK--DIR--------
	else if((isset($action)) && ($action == 'checkdir') && (isset($file)))
	{
		echo is_dir($root.$file) ? "EXIST" : "NO EXIST"; return;
	}
	else if((isset($action)) && ($action == 'checkdir') && (!isset($file)))
	{
		echo "DIR FOR CHECK NOT SET"; return;
	}
	
	//--------------------------COPY--DIR----------
	else if((isset($action)) && ($action == 'copy') && (isset($file)) && (isset($_GET['pathto'])))
	{
		if(!is_dir($root.$file)) { echo "COPY ERROR! DIR NOT EXISTS"; return; }
		if(is_dir($root.$_GET['pathto'])) { echo "COPY ERROR! DIR ALREADY EXISTS!"; return; }
		if($root.$file == $root.$_GET['pathto']) { echo "COPY ERROR! DIR NOT BE COPY IN SELF"; return; }
		if(!is_dir($root.$_GET['pathto'])) { mkdir($root.$_GET['pathto'], 0777, true); }
		CopyDir($root.$file, $root.$_GET['pathto']);
		echo is_dir($root.$_GET['pathto']) ? "COPY OK" : "COPY ERROR";
		return;
	}
	else if((isset($action)) && ($action == 'copy') && (!isset($file) || !isset($_GET['pathto'])))
	{
		echo "COPY ARGS NOT SET"; return;
	}
	
	//--------------------------SCAN--DIR-------------------
	else if((isset($action)) && ($action == 'scan') && isset($file))
	{
		if(!is_dir($root.$file)) { echo "SCAN ERROR! DIR NOT EXISTS"; return; }
		/*$dirs = array(); // ONLY DIRECTORIES
		$dir = dir($path);
		
		while (false !== ($entry = $dir->read())) {
		    if ($entry != '.' && $entry != '..') {
		       if (is_dir($path . '/' .$entry)) {
		            $dirs[] = $entry; 
		       }
		    }
		}
		
		sort($dirs);
		echo json_encode($dirs);*/
		
		$data = array_values(array_diff(scandir($root.$file), array('.', '..')));
		$result = [];
		for($i = 0; $i < count($data); $i++)
		{
			$tmp = (object)[];
			$tmp->isdir = is_dir($root.$file.$data[$i]);
			$tmp->name = $data[$i];
			array_push($result, $tmp);
		}
		echo json_encode($result);
		return;
		
		//echo json_encode(array_values(array_diff(scandir($root.$file), array('.', '..')))); //scandir dir+files, array_dif a3=a1-a2, arr val remove keys | c# problem parse this json
		return;
	}
	else if((isset($action)) && ($action == 'scan') && isset($file))
	{
		echo "SCAN ARGS NOT SET"; return;
	}
	
	
	
	
	//------------------------ZIP--------------
	else if((isset($action)) && ($action == 'zip') && (isset($file)) && (isset($_GET['path'])))
	{
		$path = $root.$_GET['path'];
		if($path[(strlen($path)-1)] == '/') { $path = rtrim($path, "/"); } // remove last char
		if(!is_dir($path)) { echo "ZIP ERROR! DIR NOT EXISTS"; return; }
		Zip($path, $root.$file);
		echo file_exists($root.$file) ? "ZIP OK" : "ZIP ERROR";
		return;
	}
	else if((isset($action)) && ($action == 'zip') && (!isset($file) || !isset($_GET['path'])))
	{
		echo "ZIP ARGS NOT SET"; return;
	}
	
	
	//--------UNZIP IN ROOT ZIPFILE
	else if((isset($action)) && ($action == 'unzip') && (isset($file)))
	{
		if(!is_file($root.$file)) { echo "UNZIP ERROR! FILE NOT EXISTS!"; return; }
		$filename = pathinfo($file)['filename'];
		$path = pathinfo($file)['dirname'];
		mkdir($root.$path, 0777, true);
		unZip($root.$file, $root.$path);
		echo is_dir($root.$path) ? "UNZIP OK" : "UNZIP ERROR"; return;
	}
	else if((isset($action)) && ($action == 'unzip') && (!isset($file)))
	{
		echo "UNZIP PATH NOT SET"; return;
	}
	
	//-------UNZIP IN DIR ZIPFILENAME
	else if((isset($action)) && ($action == 'unzipinzipname') && (isset($file)))
	{
		if(!is_file($root.$file)) { echo "UNZIPINZIPNAME ERROR! FILE NOT EXISTS!"; return; }
		$filename = pathinfo($file)['filename'];
		$path = pathinfo($file)['dirname'];
		mkdir($root.$path."/".$filename, 0777, true);
		unZip($root.$file, $root.$path."/".$filename);
		echo is_dir($root.$path."/".$filename) ? "UNZIPINZIPNAME OK" : "UNZIP ERROR"; return;
	}
	else if((isset($action)) && ($action == 'unzipinzipname') && (!isset($file)))
	{
		echo "UNZIPINZIPNAME PATH NOT SET"; return;
	}
	
	//--------UNZIP IN TARGET DIR
	else if((isset($action)) && ($action == 'unzipintarget') && (isset($file)) && (isset($_GET['toextract'])))
	{
		if(!is_file($root.$file)) { echo "UNZIPINTARGET ERROR! FILE NOT EXISTS!"; return; }
		mkdir($root.$_GET['toextract'], 0777, true);
		unZip($root.$file, $root.$_GET['toextract']);
		echo is_dir($root.$_GET['toextract']) ? "UNZIPINTARGET OK" : "UNZIP ERROR"; return;
	}
	else if((isset($action)) && ($action == 'unzipintarget') && (!isset($file) || !isset($_GET['toextract'])))
	{
		echo "UNZIPINTARGET PATH NOT SET"; return;
	}
	
	
	
	
	
	//------------------BAD----ACTION-----
	else
	{
		echo "NO SET ACTION";
		return;
		header('HTTP/1.0 403 Forbidden');
		# header("HTTP/1.1 404 Not Found");
	    /*$contents = file_get_contents('403.html');
	    exit($contents);*/
	}
	
	
	//------------------------FUNCTIONS------------------//
	
	/*------------------------ZIP---------------------------*/
	function Zip($sourcePath, $outZipPath) // ./dir1  ./dir1.zip
	{
		function folderToZip($folder, &$zipFile, $exclusiveLength)
	    {
	        $handle = opendir($folder);
	        while ($f = readdir($handle)) {
	            if ($f != '.' && $f != '..') {
	                $filePath = "$folder/$f";
	                // Remove prefix from file path before add to zip.
	                $localPath = substr($filePath, $exclusiveLength);
	                if (is_file($filePath)) {
	                    $zipFile->addFile($filePath, $localPath);
	                } elseif (is_dir($filePath)) {
	                    // Add sub-directory.
	                    $zipFile->addEmptyDir($localPath);
	                    folderToZip($filePath, $zipFile, $exclusiveLength);
	                }
	            }
	        }
	        closedir($handle);
	    }
	
		function zipDir($sourcePath, $outZipPath)
	    {
	        $pathInfo = pathInfo($sourcePath);
	        $parentPath = $pathInfo['dirname'];
	        $dirName = $pathInfo['basename'];
	
	        $z = new ZipArchive();
	        $z->open($outZipPath, ZIPARCHIVE::CREATE);
	        $z->addEmptyDir($dirName);
	        folderToZip($sourcePath, $z, strlen("$parentPath/"));
	        $z->close();
	    }
	    zipDir($sourcePath, $outZipPath);
	}
	function unZip($file, $destination)
	{
		$zip = new ZipArchive;
		if ($zip->open($file) === TRUE) {
		    $zip->extractTo($destination);
		    $zip->close();
		    if(is_dir(str_replace(".zip", "", $file))) { return true; }
		}
		return false;
	}
	/*------------------------------------------------------*/
	
	
	
	
	/*-------------------------DIRS-------------------------*/
	function deleteDir($dirPath)
	{
	    if (!is_dir($dirPath)) {
	    	return;
	        //throw new InvalidArgumentException("$dirPath must be a directory");
	    }
	    if (substr($dirPath, strlen($dirPath) - 1, 1) != '/') {
	        $dirPath .= '/';
	    }
	    $files = glob($dirPath . '*', GLOB_MARK);
		foreach ($files as $file) {
	        if (is_dir($file)) {
	            deleteDir($file);
	        } else {
	            unlink($file);
	        }
	    }
	    rmdir($dirPath);
	}
	
	function clearDir($dirPath)
	{
	    if (!is_dir($dirPath)) {
	    	return;
	        //throw new InvalidArgumentException("$dirPath must be a directory");
	    }
	    if (substr($dirPath, strlen($dirPath) - 1, 1) != '/') {
	        $dirPath .= '/';
	    }
	    $files = glob($dirPath . '*', GLOB_MARK);
		foreach ($files as $file) {
	        if (is_dir($file)) {
	            clearDir($file);
	        } else {
	            unlink($file);
	        }
	    }
	}
	
	function CopyDir($source, $dest){
    	if(!is_dir($dest)) { mkdir($dest, 0777, true); }
        if(is_dir($source)) {
            $dir_handle=opendir($source);
            while($file=readdir($dir_handle)){
                if($file!="." && $file!=".."){
                    if(is_dir($source."/".$file)){
                        if(!is_dir($dest."/".$file)){
                            mkdir($dest."/".$file);
                        }
                        CopyDir($source."/".$file, $dest."/".$file);
                    } else {
                        copy($source."/".$file, $dest."/".$file);
                    }
                }
            }
            closedir($dir_handle);
        } else {
            copy($source, $dest);
        }
    }
	/*------------------------------------------------------*/
	
	
	
	//-------------OTHER---FUNC---
	function encryptPassword($p)
	{
		if ((!isset($p)) || ($p == '')){ return null; }
	    return sha1(md5($p));
	}
	function xor_string($string, $key) {
	    for($i = 0; $i < strlen($string); $i++) 
	        $string[$i] = ($string[$i] ^ $key[$i % strlen($key)]);
	    return $string;
    }
    
    
    
?>