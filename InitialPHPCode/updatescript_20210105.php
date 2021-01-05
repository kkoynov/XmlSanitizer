 <?php

echo 'Start: '. date("Y-m-d H:i:s") ."\n";

//Load the initial document and remove all duplicates.

$xml=simplexml_load_file("/Desktop/DevPHP/files/Footshop_eu.xml");

$seen=array();

$len=$xml->entry->count();
for($i=0;$i<$len;$i++){
    $key=(string) $xml->entry[$i]->item_group_id;
    if (isset($seen[$key])) {
        unset($xml->entry[$i]);
        $len--;
        $i--;
    }else{
        $seen[$key]=1;
    }
}

$xml->asXML("/Desktop/DevPHP/files/songlist.xml");


//Load the new document and remove all nodes that are not present in the csv file with existing skus in the website.

$doc = new DOMDocument('1.0', 'UTF-8');
$doc->load("/Desktop/DevPHP/files/songlist.xml");
$entries = $doc->getElementsByTagName("entry");
$existing_skus = file('/Desktop/DevPHP/files/existingtest.csv', FILE_IGNORE_NEW_LINES);
$nodesForDeletion = array();

foreach($entries as $entry) {    
    $sku_in_xml = $entry->getElementsByTagName('item_group_id')->item(0)->nodeValue;
    $inarray = array_search($sku_in_xml, $existing_skus);	
        if ($inarray == false) { // if the item_group_id sku is not in the array of existing skus in the website
	    $nodesForDeletion[] = $entry;
        }
}

foreach($nodesForDeletion as $entry) {
    $parentnode = $entry->parentNode;
    $entry->parentNode->removechild($entry); // remove this entry
}

$doc->save("/Desktop/DevPHP/files/songlist.xml");


//Load the new document again and remove all empty lines.

$file = "/Desktop/DevPHP/files/songlist.xml";
$fp = fopen($file, "rb") or die("cannot open file");
$str = fread($fp, filesize($file));
$xml = new DOMDocument();

$xml->formatOutput = true;
$xml->preserveWhiteSpace = false;
$xml->loadXML($str) or die("Error");

file_put_contents("/Desktop/DevPHP/files/songlist.xml", $xml->saveXML());

echo 'End:   '. date("Y-m-d H:i:s") ."\n";


?>
