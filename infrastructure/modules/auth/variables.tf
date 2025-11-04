variable "name" {
	type    = string
	default = "sarc"
}

variable "password_policy" {
	description = "Optional password policy"
	type = object({
		minimum_length    = number
		require_lowercase = bool
		require_uppercase = bool
		require_numbers   = bool
		require_symbols   = bool
	})
	default = {
		minimum_length    = 8
		require_lowercase = true
		require_uppercase = true
		require_numbers   = true
		require_symbols   = false
	}
}

