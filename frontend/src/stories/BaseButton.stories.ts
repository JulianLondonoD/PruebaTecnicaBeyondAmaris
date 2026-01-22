import type { Meta, StoryObj } from '@storybook/vue3'
import BaseButton from '@/components/ui/BaseButton.vue'

const meta: Meta<typeof BaseButton> = {
  title: 'UI/BaseButton',
  component: BaseButton,
  tags: ['autodocs'],
  argTypes: {
    variant: {
      control: 'select',
      options: ['primary', 'secondary', 'danger', 'success', 'outline'],
      description: 'The visual style of the button'
    },
    size: {
      control: 'select',
      options: ['sm', 'md', 'lg'],
      description: 'The size of the button'
    },
    type: {
      control: 'select',
      options: ['button', 'submit', 'reset'],
      description: 'The HTML button type'
    },
    disabled: {
      control: 'boolean',
      description: 'Whether the button is disabled'
    },
    loading: {
      control: 'boolean',
      description: 'Whether the button is in a loading state'
    },
    onClick: {
      action: 'clicked'
    }
  },
  args: {
    variant: 'primary',
    size: 'md',
    type: 'button',
    disabled: false,
    loading: false
  }
}

export default meta
type Story = StoryObj<typeof BaseButton>

export const Primary: Story = {
  args: {
    variant: 'primary'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Primary Button</BaseButton>'
  })
}

export const Secondary: Story = {
  args: {
    variant: 'secondary'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Secondary Button</BaseButton>'
  })
}

export const Danger: Story = {
  args: {
    variant: 'danger'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Danger Button</BaseButton>'
  })
}

export const Success: Story = {
  args: {
    variant: 'success'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Success Button</BaseButton>'
  })
}

export const Outline: Story = {
  args: {
    variant: 'outline'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Outline Button</BaseButton>'
  })
}

export const Small: Story = {
  args: {
    size: 'sm'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Small Button</BaseButton>'
  })
}

export const Large: Story = {
  args: {
    size: 'lg'
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Large Button</BaseButton>'
  })
}

export const Disabled: Story = {
  args: {
    disabled: true
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Disabled Button</BaseButton>'
  })
}

export const Loading: Story = {
  args: {
    loading: true
  },
  render: args => ({
    components: { BaseButton },
    setup() {
      return { args }
    },
    template: '<BaseButton v-bind="args">Loading Button</BaseButton>'
  })
}

export const AllVariants: Story = {
  render: () => ({
    components: { BaseButton },
    template: `
      <div class="space-y-4">
        <div class="space-x-2">
          <BaseButton variant="primary">Primary</BaseButton>
          <BaseButton variant="secondary">Secondary</BaseButton>
          <BaseButton variant="danger">Danger</BaseButton>
          <BaseButton variant="success">Success</BaseButton>
          <BaseButton variant="outline">Outline</BaseButton>
        </div>
        <div class="space-x-2">
          <BaseButton size="sm">Small</BaseButton>
          <BaseButton size="md">Medium</BaseButton>
          <BaseButton size="lg">Large</BaseButton>
        </div>
        <div class="space-x-2">
          <BaseButton disabled>Disabled</BaseButton>
          <BaseButton loading>Loading</BaseButton>
        </div>
      </div>
    `
  })
}
