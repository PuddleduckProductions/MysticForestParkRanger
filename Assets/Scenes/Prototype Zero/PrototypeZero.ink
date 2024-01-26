VAR interacted_once = false
=== interact_Dugong ===
{
	- interacted_once == false:
		Dugong:A #symbols
		~ interacted_once = true
	- else:
		Dugong:B #symbols
		$moveTo #Dugong #Player
		Dugong:D #symbols
		$moveTo #Dugong #(0, 0, 0)
 }
-> END