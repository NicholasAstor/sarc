output "vpc_id" {
  description = "ID of the VPC"
  value       = module.vpc.vpc_id
}

output "ec2_public_ip" {
  description = "Public IP address of the EC2 instance"
  value       = module.ec2.public_ip
}

output "ec2_public_dns" {
  description = "Public DNS name of the EC2 instance"
  value       = module.ec2.public_dns
}

output "rds_endpoint" {
  description = "RDS instance endpoint"
  value       = module.database.db_endpoint
  sensitive   = true
}

output "application_url" {
  description = "URL to access the application"
  value       = "http://${module.ec2.public_ip}:5000"
}
