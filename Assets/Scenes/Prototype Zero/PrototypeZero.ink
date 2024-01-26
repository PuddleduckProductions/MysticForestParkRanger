VAR interacted_once = false
=== interact_Dugong ===
{
	- interacted_once == false:
		Dugong:A #symbols
		Dugong: You will not recieve this dialog again. WooooOooOOOOo
		~ interacted_once = true
	- else:
		Dugong:B #symbols
		Dugong: He he gottem
		$moveTo #Dugong #Player
		Dugong:D #symbols
		$moveTo #Dugong #(0, 0, 0)
 }
-> END