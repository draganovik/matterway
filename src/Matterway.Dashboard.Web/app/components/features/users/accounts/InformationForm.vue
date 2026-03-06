<script setup lang="ts">
type SystemUserForm = {
  email: string
  password: string
}

const { disabled = false } = defineProps<{
  disabled?: boolean
}>()

const form = defineModel<SystemUserForm>({ required: true })
</script>

<template>
  <div class="grid gap-4">
    <UFormField label="Email" help="Optional. Leave unchanged if empty.">
      <UInput
        v-model="form.email"
        type="email"
        placeholder="name@matterway.local"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

    <UFormField
      label="Password"
      help="Optional. Must be at least 6 characters when provided."
    >
      <UInput
        v-model="form.password"
        type="password"
        placeholder="New password"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>
  </div>
</template>
