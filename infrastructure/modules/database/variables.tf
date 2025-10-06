variable "db_username" {
  description = "Username for the RDS instance"
  type        = string
}

variable "db_password" {
  description = "Password for the RDS instance"
  type        = string
  sensitive   = true
}

variable "db_name" {
  description = "Name of the RDS database"
  type        = string
}

variable "rds_security_group_id" {
  description = "Security group ID for RDS"
  type        = string
}

variable "private_subnet_ids" {
  description = "IDs of private subnets"
  type        = list(string)
}