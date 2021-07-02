  UPDATE [WarningRule] SET [Description] = REPLACE([Description],'Patient {Patient}','A Patient')
  UPDATE [WarningRule] SET [Title] = REPLACE([Title],'Patient {Patient}','A Patient')
  UPDATE WarningRuleDefault SET [Description] = REPLACE([Description],'Patient {Patient}','A Patient')
  UPDATE WarningRuleDefault SET [Title] = REPLACE([Title],'Patient {Patient}','A Patient')