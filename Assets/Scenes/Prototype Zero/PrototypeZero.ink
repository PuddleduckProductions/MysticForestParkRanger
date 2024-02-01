VAR interacted_once = false
=== interact_Dugong ===
{
	- interacted_once == false:
		Dugong:H #symbols
		Dugong: You will not recieve this dialog again. WooooOooOOOOo
		~ interacted_once = true
	- else:
		Dugong:E #symbols
		Dugong: He he gottem
		$moveTo #Dugong #Player
		Dugong:W #symbols
		$moveTo #Dugong #(0, 0, 0)
 }
-> END