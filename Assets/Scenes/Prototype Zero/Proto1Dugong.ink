VAR interacted_once = false
=== interact_Dugong ===
{
	- interacted_once == false:
		Dugong:H #symbols
		~ interacted_once = true
	- else:
		Dugong:E #symbols
		Dugong: He he gottem
		$moveTo #Dugong #Player
		Dugong:W #symbols
		$moveTo #Dugong #(0, 0, 0)
 }
-> END