resource "aws_cognito_user_pool" "this" {
	name = "${var.name}-user-pool"

	password_policy {
		minimum_length    = var.password_policy.minimum_length
		require_lowercase = var.password_policy.require_lowercase
		require_uppercase = var.password_policy.require_uppercase
		require_numbers   = var.password_policy.require_numbers
		require_symbols   = var.password_policy.require_symbols
	}

	schema {
		name = "email"
		attribute_data_type = "String"
		required = true
		developer_only_attribute = false
		mutable = true
	}

	account_recovery_setting {
		recovery_mechanism {
			name     = "verified_email"
			priority = 1
		}
	}

	tags = {
		Name = "${var.name}-user-pool"
		Terraform = "true"
	}
}

resource "aws_cognito_user_pool_client" "app_client" {
	name         = "${var.name}-app-client"
	user_pool_id = aws_cognito_user_pool.this.id

	explicit_auth_flows = [
		"ALLOW_USER_PASSWORD_AUTH",
		"ALLOW_REFRESH_TOKEN_AUTH",
		"ALLOW_USER_SRP_AUTH",
		"ALLOW_CUSTOM_AUTH"
	]

	generate_secret = false

	prevent_user_existence_errors = "ENABLED"

	allowed_oauth_flows = []
	allowed_oauth_scopes = []
	callback_urls = []
	logout_urls = []
}

